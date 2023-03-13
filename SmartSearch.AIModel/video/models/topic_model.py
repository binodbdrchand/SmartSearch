import gensim
import json
import os
import pandas as pd
import pickle

from gensim.models import CoherenceModel

from config import model_config, output_config
from file_logger import get_logger


os.environ['KMP_DUPLICATE_LIB_OK'] = 'True'
lg = get_logger()


class TopicModel:
    def __init__(self, dictionary, corpus, texts, preprocessed_data, file_name, total_language, file_name_only,
                 frame_information):
        """

        :param dictionary:
        :param corpus:
        :param texts:
        :param preprocessed_data:
        :param file_name:
        """
        self.lda = None
        self.lda_model = None
        self.dictionary = dictionary
        self.corpus = corpus
        self.text = texts
        self.preprocessed_data = preprocessed_data
        self.file_name = file_name
        with open(os.path.dirname(__file__) + '../../config/params.json', 'r') as file:
            params = json.load(file)
        self.topic = params['topic']
        self.chunk_size = params['chunk_size']
        self.passes = params['passes']
        self.iterations = params['iterations']
        self.alpha = params['alpha']
        self.eta = params['eta']
        self.random_state = params['random_state']
        self.num_words = params['num_words']
        self.top_n_prob_words = params['top_n_prob_words']
        self.eval_every = None
        self.coherence_score = None
        self.perplexity = None
        self.coherence_model = None
        self.num_topics = None
        self.LDAvis_prepared = None
        self.topics_df = None
        self.model = None
        self.loaded_model = None
        self.predicted_model = None
        self.BOW = None
        self.total_language = total_language
        self.file_name_only = file_name_only
        self.frame_information = frame_information
        # self.model_fit = self.model_fit()

    def model_fit(self):
        """
        This function fits the corpus and dictionary into the LDA topic model and provides the topics and words
        :return: Required topics  and words from the corpus,evaluation matrix of lda model
        """
        try:

            self.lda = gensim.models.ldamodel.LdaModel
            self.lda_model = self.lda(corpus=self.corpus, id2word=self.dictionary, num_topics=self.topic,
                                      chunksize=self.chunk_size, passes=self.passes, random_state=self.random_state,
                                      eval_every=self.eval_every, alpha=self.alpha, eta=self.eta,
                                      iterations=self.iterations)

            # Extracting topics from the corpus
            for idx, topics in self.lda_model.print_topics(num_words=self.num_words, num_topics=self.topic):
                lg.info("Topic: {a} \n Words: {b}".format(a=idx, b=topics))

            # printing the topic associating with its documents
            count = 1
            for i in self.lda_model[self.corpus]:
                lg.info("document: {a}{b}".format(a=count, b=i))
                # print("document:", count, i)
                count += 1

            # Performing model performance (lower the Perplexity value, better the model)
            self.coherence_model = CoherenceModel(model=self.lda_model, texts=self.text, dictionary=self.dictionary,
                                                  coherence='c_v')
            self.coherence_score = self.coherence_model.get_coherence()
            lg.info("Coherence_score: %s", self.coherence_score)
            self.perplexity = self.lda_model.log_perplexity(self.corpus)  # lower the Perplexity value, better the model
            lg.info("Perplexity: %s", self.perplexity)

            return self.coherence_score, self.perplexity
        except Exception as e:
            lg.exception((str(e)))

    def model_prediction(self):
        try:
            # load the saved model  from the directory
            script_dir = os.path.dirname(os.path.abspath(__file__))
            config_path = os.path.join(script_dir, model_config.MODEL_FILE_NAME)

            self.loaded_model = pickle.load(open(config_path, 'rb'))
            lg.info("Saved model %s", self.loaded_model)

            # load the saved trained dictionary from root directory
            root_dir = os.path.dirname(os.path.abspath(__file__)).rsplit(os.sep, 1)[0]
            config_path = os.path.join(root_dir, model_config.DICTIONARY_FILE_NAME)
            trained_dictionary = pickle.load(open(config_path, 'rb'))

            # Bag of words of new unseen documents
            self.BOW = [trained_dictionary.doc2bow(i) for i in self.text]
            # print(self.BOW)
            self.predicted_model = self.loaded_model[self.BOW]
            # calculating the Perplexity of saved model for model performance, lower the better
            perplexity_saved_model = self.loaded_model.log_perplexity(self.BOW)
            lg.info('Perplexity of saved model: %s', perplexity_saved_model)
        except Exception as e:
            lg.exception((str(e)))

    def get_dominant_topics(self):
        """
        This function determines what topic a given document. Here, we will find that topic number
         which has highest percentage contribution in that particular document.
        :return: Dominant topics of all document along the words.
        """
        try:
            # initialize output
            self.topics_df = pd.DataFrame()

            # Get the main topics in each document
            for i, row in enumerate(self.loaded_model[self.BOW]):
                row = sorted(row, key=lambda x: (x[1]), reverse=True)
                # Get the dominant topic ,% contribution and key words for each document
                for j, (topic_num, prop_topic) in enumerate(row):
                    if j == 0:  # dominant topic
                        key = self.loaded_model.show_topic(topic_num, topn=50)
                        topic_keywords = ",".join([word for word, prop in key])
                        self.topics_df = self.topics_df.append(pd.Series([topic_num, round(prop_topic, 4),
                                                                          topic_keywords]), ignore_index=True)
                    else:
                        break
            self.topics_df.columns = ['TopicNumber', 'Percentage_Contribution', 'Topic_Keywords']
            self.topics_df['TopicNumber'] = self.topics_df['TopicNumber'].astype(int)

            # Add original text to the end of output
            df_preprocessed_data = pd.DataFrame({'preprocessed_data': self.preprocessed_data})
            contents = df_preprocessed_data['preprocessed_data'].apply(lambda x: ','.join(x.split()))

            file_name = pd.Series(self.file_name_only)
            total_language = pd.Series(self.total_language)
            df_topic_sentences_keywords = pd.concat([self.topics_df, file_name, total_language], axis=1)

            # Format
            df_dominant_topics = df_topic_sentences_keywords.reset_index()
            df_dominant_topics.columns = ['VideoNo', 'TopicNumber', 'TopicProbability', 'Keywords',
                                          'VideoName', 'Language']
            df_topic_keyword = df_dominant_topics[['VideoName', 'TopicNumber', 'TopicProbability',
                                                   'Keywords', 'Language']]

            file_location = {'VideoName': self.file_name_only, 'VideoLocation': self.file_name}
            df_file = pd.DataFrame(file_location)
            final_output = df_file.merge(df_topic_keyword, on="VideoName", how='left')

            # concating the video frame dataframe and final output
            clip_data = self.frame_information.rename(
                columns={'clip_text': 'ClipText',
                         'clip_start_time': 'ClipStart',
                         'clip_duration': 'ClipDuration'})
            final_output = pd.concat([final_output, clip_data], axis=1)
            root_dir = os.path.dirname(os.path.abspath(__file__)).rsplit(os.sep, 1)[0]
            json_output_path = os.path.join(root_dir,
                                            output_config.VIDEO_OUTPUT_JSON_FILE_LOCATION,
                                            output_config.VIDEO_OUTPUT_JSON_FILE_NAME)
            json_response = json.loads(final_output.to_json(orient='records'))
            with open(json_output_path, "w", encoding="utf8") as outfile:
                json.dump(json_response, outfile, ensure_ascii=False)
            lg.info('Required Json File is successfully created!!')
            lg.info("Model has successfully performed the work !!")

            return json_response
        except Exception as e:
            lg.exception((str(e)))
