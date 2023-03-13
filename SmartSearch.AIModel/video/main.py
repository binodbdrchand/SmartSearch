import multiprocessing
import sys

from multiprocessing import freeze_support
from multiprocessing import Pool as ProcessPool

from file_logger import get_logger
from models import topic_model
from transformations import data_transformation
from utils import file_handling, multitasking, preprocessing


lg = get_logger()


def main():
    try:
        file = [sys.argv[1]]
        file_list, file_name_only = file_handling.file_path(file)
        lg.info("total no .of files: %s", len(file_list))
        lg.info("file_name_only: %s", file_name_only)

        num_process = multiprocessing.cpu_count()
        lg.info("num of processor %s", num_process)

        with ProcessPool(processes=num_process) as pool:
            results = pool.map(multitasking.batch_processing, file_list)
        lg.info("Multiprocessing work is completed !!")
        combined_data = [' '.join(i[0]) for i in results]
        lg.info("length of combined_data:%s", len(combined_data))
        frame_information = multitasking.data_frame(results)
        preprocessed_data, total_language = preprocessing.preprocessing(combined_data)
        bi_gram_data = preprocessing.bi_gram_data(preprocessed_data)
        dictionary, bag_of_words, final_corpus, = data_transformation.doc_term_matrix(bi_gram_data)
        lg.info("Data is fitted to the Model !!")
        model = topic_model.TopicModel(dictionary, bag_of_words, final_corpus, preprocessed_data, file_list,
                                       total_language, file_name_only, frame_information)

        lg.info("Model is predicting new video files")
        prediction = model.model_prediction()
        lg.info("Prediction work is done !!")

        # Extracting the dominant topic from each document with necessary features
        dominant_topic = model.get_dominant_topics()

        return dominant_topic
    except Exception as e:
        print("File path is Invalid.Please Try Again!! ", e)
        lg.exception((str(e)))


if __name__ == '__main__':
    freeze_support()
    json_response = main()
    print(json_response[0])