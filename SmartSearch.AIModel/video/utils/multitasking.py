import os
from multiprocessing import current_process
import cv2
import pytesseract
from nltk.tokenize import sent_tokenize
import moviepy.editor as mp
import threading
import datetime
from configparser import ConfigParser
import azure.cognitiveservices.speech as speechsdk
import json
import time
import pandas as pd
from file_logger import get_logger

lg = get_logger()


FRAME_RATE = 2
pytesseract.pytesseract.tesseract_cmd = os.path.join(os.path.dirname(os.path.abspath(__file__)) +
                                                     r'\Tesseract-OCR\tesseract.exe')


def visual_to_text(file_name):
    """
    This function converts each video file into images based on frame rate.
    The text data is extracted from the converted images.

    :param file_name: Video file as input parameter
    :return:list of text data extracted from each video file
    """
    try:
        lg.info("Video file for image")
        lg.info("file name %s", file_name)
        lg.info('threading Starting %s', threading.currentThread().getName())
        text_data = []
        sentence_text = []
        final_text = []

        # To print the process id
        process_id = os.getpid()
        lg.info("process_id %s", process_id)
        # To print the process_name
        process_name = current_process().name
        lg.info("process_name %s", process_name)

        global FRAME_RATE, FRAME_TIME, FRAME_COUNT

        combined_text = " "
        # Read the video from specified path
        video_data = cv2.VideoCapture(file_name)
        video_length = int(video_data.get(cv2.CAP_PROP_FRAME_COUNT)) - 1  # video length
        fps = int(video_data.get(cv2.CAP_PROP_FPS))   # frame per second
        pos_frame = int(video_data.get(cv2.CAP_PROP_POS_FRAMES))  # position of video frame
        width = int(video_data.get(cv2.CAP_PROP_FRAME_WIDTH))   # width of video frame
        height = int(video_data.get(cv2.CAP_PROP_FRAME_HEIGHT))  # height of video frame

        FRAME_TIME = 0
        FRAME_COUNT = 0
        video_time = int(video_data.get(cv2.CAP_PROP_POS_MSEC))
        lg.info("total_frames: %s", video_length)
        lg.info("FRAME_RATE: %s", FRAME_RATE)
        lg.info("frame_per_second:%s ", fps)
        lg.info("pos_frame: %s", pos_frame)
        lg.info("width: %s", width)
        lg.info("height: %s", height)
        lg.info("time: %s", video_time)

        # Save the images into the directory
        root_dir = os.path.dirname(os.path.abspath(__file__)).rsplit(os.sep, 1)[0]
        dir_name = "image_dir"

        file_name_as_string = ""
        file_name_as_string = file_name_as_string.join(file_name)
        last_index_of = file_name_as_string.rfind('/')
        total_length = len(file_name_as_string)
        file_name_only = file_name_as_string[last_index_of+1:total_length]
        image_dir = os.path.join(root_dir, dir_name, datetime.datetime.now().strftime('%H-%M-%S_visual_'
                                                                                      + file_name_only))
        try:
            if not os.path.isdir(image_dir):
                os.makedirs(image_dir)
        except OSError:
            lg.info('Create directory.' + image_dir)

        while True:
            video_data.set(cv2.CAP_PROP_POS_MSEC, FRAME_TIME * 10000)  # move frame to a timestamp
            FRAME_TIME += 1 / FRAME_RATE
            # reading  from frame
            ret, frame = video_data.read()

            if ret:
                # if video is still left continue creating images
                image = (r'frame' + file_name_only + str(FRAME_COUNT) + '.jpg')

                # writing the extracted images
                cv2.imwrite((os.path.join(image_dir, image)), frame)

                # cv2.imwrite((image), frame)

                if image is not None:
                    # Extracting the text from each video frame using OCR
                    text = pytesseract.image_to_string(os.path.join(image_dir, image), lang='eng+deu+fra')
                    # print(text)
                    combined_text += text

                FRAME_COUNT += 1

            else:
                break

        # adding all the  sentences of each video file into  the list.
        text_data.append(combined_text)

        # Release all space and windows once done
        video_data.release()

        # Removing the duplicate sentences from the list.
        for i in range(len(text_data)):
            data = []
            for sentence in list(sent_tokenize(text_data[i])):
                if sentence not in data:
                    (data.append(sentence))
                sentence_text = (''.join(data))
            final_text.append(sentence_text)
        lg.info("visual work done %s", file_name)

        return final_text
    except Exception as e:
        lg.exception((str(e)))


def speech_to_text(file_name):
    """
    This function converts the video file into audio file and extract the text using azure continuous speech recognition
    method
    :param file_name: video file as input parameter
    :return:  list of text data, words and their time information  extracted from each video file

    """
    try:

        lg.info("Video file for audio")
        lg.info("file_name %s", file_name)
        lg.info('Processor starting %s', threading.currentThread().getName())

        #To print the process id
        process_id = os.getpid()
        lg.info("process_id %s", process_id)

        # To print the process_name
        process_name = current_process().name
        lg.info("process_name %s", process_name)

        # Read the video file
        clip = mp.VideoFileClip(file_name)
        duration = clip.duration
        lg.info(" The duration of the video {a}is {b}".format(a=file_name, b=duration))

        # Saved the audio files  into the directory
        root_dir = os.path.dirname(os.path.abspath(__file__)).rsplit(os.sep, 1)[0]
        dir_name = "audio_dir"

        file_name_as_string = ""
        file_name_as_string = file_name_as_string.join(file_name)
        last_index_of = file_name_as_string.rfind('/')
        total_length = len(file_name_as_string)
        file_name_only = file_name_as_string[last_index_of + 1:total_length]
        audio_dir = os.path.join(root_dir, dir_name,
                                 datetime.datetime.now().strftime('%H-%M-%S_audio_' + file_name_only))

        # Converting the video file into  .wave audio file
        wav_file = audio_dir + "converted.wav"
        clip.audio.write_audiofile(wav_file, logger=None)

        def speech_continous(wav_file):
            """
            This function will continously extarct the text,video data information using azure api
            based on trigerring event on video file.
            :param wav_file: Converted audio file from video
            :return: ist of text data, words and their time information  extracted from each video file
            """
            try:
                # Configuring the config file to read the credentials
                config_file = r'config/Videosconfig.ini'
                config_object = ConfigParser()
                config_object.read(os.path.join(root_dir, config_file))
                credentials = config_object['Microsoft_credentials']
                speech_key = credentials["speech_key"]
                service_region = credentials["service_region"]

                # Creates an speech  configuration using Microsoft credentials
                speech_config = speechsdk.SpeechConfig(subscription=speech_key, region=service_region)

                # Creates an audio configuration that points to an audio file.
                # print("wav_file:", wav_file)
                lg.info("audio_wav_file %s", wav_file)

                audio_config = speechsdk.audio.AudioConfig(filename=wav_file)

                # Creates a recognizer with the given settings
                # speech_config.speech_recognition_language = "de-DE"
                speech_config.request_word_level_timestamps()

                speech_config.enable_dictation()
                speech_config.output_format = speechsdk.OutputFormat(1)

                # To detect the speech language automatically

                auto_detect_source_language_config = speechsdk.languageconfig.AutoDetectSourceLanguageConfig(
                     languages=["en-US", "de-DE", "zh-CN"])
                speech_recognizer = speechsdk.SpeechRecognizer(
                    speech_config=speech_config, auto_detect_source_language_config=auto_detect_source_language_config,
                    audio_config=audio_config)
                '''
                speech_recognizer = speechsdk.SpeechRecognizer(speech_config=speech_config,
                                                               audio_config=audio_config)
                '''
                # Variable to monitor status
                done = False

                # Service callback that stops continuous recognition upon receiving an event `evt`
                def stop_cb(evt):
                    speech_recognizer.stop_continuous_recognition()
                    nonlocal done
                    done = True

                all_results = []
                results = []
                transcript = []
                words = []
                clip_offset = []
                clip_duration = []

                sentence_text = []
                # https://docs.microsoft.com/en-us/python/api/azure-cognitiveservices-speech/azure.cognitiveservices.speech.recognitionresult?view=azure-python

                def handle_final_result(evt):

                    all_results.append(evt.result.text)
                    results = json.loads(evt.result.json)
                    transcript.append(results['DisplayText'])
                    confidence_list_temp = [item.get('Confidence') for item in results['NBest']]
                    max_confidence_index = confidence_list_temp.index(max(confidence_list_temp))
                    words.extend(results['NBest'][max_confidence_index]['Words'])
                    sentence_text.extend([results['DisplayText']])
                    clip_offset.extend([results['Offset'] / 10000000])
                    clip_duration.extend([results['Duration'] / 10000000])

                # Appends the recognized text to the all_results variable
                speech_recognizer.recognized.connect(handle_final_result)

                # Connect callbacks to the events fired by the speech recognizer
                speech_recognizer.recognizing.connect(lambda evt: evt)
                speech_recognizer.recognized.connect(lambda evt: evt)
                speech_recognizer.session_started.connect(lambda evt: evt)
                speech_recognizer.session_stopped.connect(lambda evt: evt)
                speech_recognizer.canceled.connect(lambda evt: evt)

                # Stop continuous recognition on either session stopped or canceled events
                speech_recognizer.session_stopped.connect(stop_cb)
                speech_recognizer.canceled.connect(stop_cb)

                lg.info("Initiating speech to text")
                speech_recognizer.start_continuous_recognition()

                while not done:
                    time.sleep(.5)

                whole_text = ""
                lg.info(("length of all result:", len(all_results)))

                # For word and its start and duration
                list_word = [word['Word'] for word in words]
                word_offset = [word['Offset'] / 10000000 for word in words]
                word_time_duration = [word['Duration'] / 10000000 for word in words]

                # For sentence
                total_sentences = sentence_text
                total_clip_offset = clip_offset
                total_clip_duration = clip_duration

                data_list = []
                for k in range(len(all_results)):
                    whole_text += all_results[k]
                data_list.append(whole_text)

                return data_list, total_sentences, total_clip_offset, total_clip_duration,\
                       list_word, word_offset, word_time_duration, file_name
            except Exception as e:
                lg.exception((str(e)))

        return speech_continous(wav_file)
    except Exception as e:
        lg.exception((str(e)))


def batch_processing(filename):

    """
    This function chunks  the video files for multiprocessing  and passed it to both function to extract the text.
    :param filename:list of video file
    :return: Combined text data from each video file
    """
    try:
        visual_processing = visual_to_text(filename)

        # applying azure speech recognition for time video frame extraction
        audio_processing, total_sentences, total_clip_offset, total_clip_duration, list_word, word_offset, \
        word_time_duration, file_name = speech_to_text(filename)

        # combined the text data from audio and visual
        combined_data = audio_processing + visual_processing
        combined_sentences = total_sentences
        combined_clip_offset = total_clip_offset
        combined_clip_duration = total_clip_duration
        combined_word = list_word
        combined_word_offset = word_offset
        combined_word_duration = word_time_duration
        combined_file_name = file_name
        final_list = [combined_data, combined_word, combined_word_offset, combined_word_duration,
                      combined_sentences, combined_clip_offset, combined_clip_duration, combined_file_name]

        return final_list
    except Exception as e:
        lg.exception(str(e))


def data_frame(results):
    """
    This function will create the dataframe for the  video data information based on time frame and store in json file
    :param results: The list of video data and their information
    :return:dataframe of each video file information
    """
    try:
        # Creating the list of each parameter of each video data
        list_word = [i[1] for i in results]
        word_offset = [i[2] for i in results]
        word_time_duration = [i[3] for i in results]
        total_sentences = [i[4] for i in results]
        total_clip_offset = [i[5] for i in results]
        total_clip_duration = [i[6] for i in results]
        total_file = [i[7] for i in results]

        # Converting the list into Series to create dataframe
        # word_start_time = pd.Series(word_offset)
        # duration = pd.Series(word_time_duration)
        # word = pd.Series(list_word)
        sentences = pd.Series(total_sentences)
        clip_start = pd.Series(total_clip_offset)
        clip_duration = pd.Series(total_clip_duration)
        # video_file = pd.Series(total_file)
        time_frame = pd.concat([sentences, clip_start, clip_duration], axis=1)
        time_frame.columns = ['clip_text', 'clip_start_time', 'clip_duration']
        lg.info('Required Json File is successfully created!!')
        return time_frame
    except Exception as e:
        lg.exception(str(e))
