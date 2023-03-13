import gensim
import json
import logging as lg
import os
import pandas as pd
import pickle
import pyLDAvis
import pyLDAvis.gensim as models
import shutup
import warnings

from config import model_config, output_config
from file_logger import get_logger
from gensim.models import CoherenceModel

lg = get_logger()
shutup.please()
#warnings.filterwarnings("ignore", category=DeprecationWarning)


class TopicModel:

    def __init__(self, dictionary, corpus, texts, preprocessed_data,
                 file_name, total_language, file_name_only, list_text,
                 list_page, list_doc):
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
        self.topic = 20
        self.chunksize = 100
        self.passes = 80
        self.iterations = 700
        self.alpha = 'auto'
        self.eta = 'auto'
        self.random_state = 2
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
        self.bag_of_words = None
        self.total_language = total_language
        self.file_name_only = file_name_only
        self.page_text = list_text
        self.page = list_page
        self.doc = list_doc

    def model_fit(self):
        try:
            """
            This function fits the corpus and dictionary into the LDA topic model and provides the topics and words
            :return: Required topics from the corpus
            """
            self.lda = gensim.models.ldamodel.LdaModel

            self.lda_model = self.lda(
                corpus=self.corpus, id2word=self.dictionary,
                num_topics=self.topic, chunksize=self.chunksize,
                passes=self.passes, random_state=self.random_state,
                eval_every=self.eval_every, alpha=self.alpha,
                eta=self.eta, iterations=self.iterations)

            # save the model to disk# root_dir = os.path.dirname(os.path.abspath(__file__))
            # config_path = os.path.join(root_dir, 'lda_model.sav')
            # pickle.dump(self.lda_model, open(config_path, 'wb'))

            # Extracting topics from the corpus
            for idx, topics in self.lda_model.print_topics(num_words=50, num_topics=20):
                lg.info("Topic: {a} \n Words: {b}".format(a=idx, b=topics))

            # Performing model performance (lower the Perplexity value, better the model)
            self.coherence_model = CoherenceModel(
                model=self.lda_model,
                texts=self.text,
                dictionary=self.dictionary,
                coherence='c_v')
            self.coherence_score = self.coherence_model.get_coherence()
            lg.info("Coherence_score: %s", self.coherence_score)

            self.perplexity = self.lda_model.log_perplexity(self.corpus)
            lg.info("Perplexity: %s", self.perplexity)

            return self.coherence_score, self.perplexity
        except Exception as e:
            print("Model could not learn from the data.Please Try Again!! ", e)
            lg.exception((str(e)))

    def model_prediction(self):
        try:
            # Load the saved model  from the directory
            script_dir = os.path.dirname(os.path.abspath(__file__))
            config_path = os.path.join(script_dir, model_config.MODEL_FILE_NAME)

            self.loaded_model = pickle.load(open(config_path, 'rb'))
            lg.info("Saved model %s", self.loaded_model)

            # load the saved trained dictionary from root directory
            config_path = os.path.join(script_dir, model_config.DICTIONARY_FILE_NAME)
            trained_dictionary = pickle.load(open(config_path, 'rb'))
            self.bag_of_words = [trained_dictionary.doc2bow(i) for i in self.text]
            self.predicted_model = self.loaded_model[self.bag_of_words]

            # calculating the Perplexity of saved model for model performance, lower the better
            perplexity_saved_model = self.loaded_model.log_perplexity(self.bag_of_words)
            lg.info('Perplexity of saved model: %s', perplexity_saved_model)
        except Exception as e:
            print("Model could not predict new  data.Please Try Again!! ", e)
            lg.exception((str(e)))

    def get_dominant_topics(self):
        """
        The function determines what topic a given document is about. Here, we will find that topic number which has the
         highest percentage contribution in that particular document
        :return: Dominant topics of all documents along with words.
        """
        try:
            # Initialize output
            self.topics_df = pd.DataFrame()

            # Get the main topics in each document
            # for i, row in enumerate(self.lda_model[self.corpus]):
            for i, row in enumerate(self.loaded_model[self.bag_of_words]):
                row = sorted(row, key=lambda x: (x[1]), reverse=True)

                # Get the dominant topics,% contribution and keywords for each document
                for j, (topic_num, prop_topic) in enumerate(row):
                    if j == 0:  # dominant topic
                        key = self.loaded_model.show_topic(topic_num, topn=100)

                        topic_keywords = ",".join([word for word, prop in key])

                        self.topics_df = self.topics_df.append(
                            pd.Series([
                                topic_num, round(prop_topic, 4), topic_keywords
                            ]),
                            ignore_index=True)
                    else:
                        break

            self.topics_df.columns = ['TopicNumber', 'Percentage_Contribution', 'Topic_Keywords']
            self.topics_df['TopicNumber'] = self.topics_df['TopicNumber'].astype(int)

            #  Split the  original text into comma separated words and add  to the end of output
            df_preprocessed_data = pd.DataFrame({'preprocessed_data': self.preprocessed_data})
            contents = df_preprocessed_data['preprocessed_data'].apply(lambda x: ','.join(x.split()))

            file_name = pd.Series(self.file_name_only)
            total_language = pd.Series(self.total_language)
            df_topic_sent_keywords = pd.concat([self.topics_df, file_name, contents, total_language], axis=1)

            df_dominant_topics = df_topic_sent_keywords.reset_index()
            df_dominant_topics.columns = ['DocumentNo', 'TopicNumber', 'TopicProbability',
                                          'Keywords', 'DocumentName', 'Corpus', 'Language']
            df_topic_keyword = df_dominant_topics[['DocumentName', 'TopicNumber', 'TopicProbability',
                                                   'Keywords', 'Corpus', 'Language']]

            file_location = {'DocumentName': self.file_name_only, 'DocumentLocation': self.file_name}
            df_file = pd.DataFrame(file_location)
            final_output = df_file.merge(df_topic_keyword, on="DocumentName", how='left')

            page_text = pd.Series(self.page_text)
            page = pd.Series(self.page)
            docs = pd.Series(self.doc)
            testing_data_frame = pd.concat([docs, page_text, page], axis=1)
            testing_data_frame.columns = ['DocumentName', 'Text', 'PageNumber']

            root_dir = os.path.dirname(os.path.abspath(__file__)).rsplit(os.sep, 1)[0]
            json_output_path = os.path.join(root_dir, output_config.DOCUMENT_OUTPUT_JSON_FILE_LOCATION,
                                            output_config.DOCUMENT_OUTPUT_JSON_FILE_NAME)
            json_response = json.loads(final_output.to_json(orient="records"))
            with open(json_output_path, "w", encoding="utf8") as outfile:
                json.dump(json_response, outfile, ensure_ascii=False)

            lg.info('Required Json File is successfully created!!')
            lg.info("Model has successfully performed the work !!")

            return json_response
        except Exception as e:
            lg.exception(str(e))

    def model_visualization(self):
        try:
            self.num_topics = len(self.lda_model.print_topics())
            ld_avis_data_filepath = os.path.join('lavs_prepared_train' + str(self.num_topics))
            ## this is a bit time-consuming - make the if statement True
            ## if you want to execute visualization prep yourself
            if 1 == 1:
                self.LDAvis_prepared = models.prepare(self.lda_model, self.corpus, self.dictionary)
                pyLDAvis.display(self.LDAvis_prepared)
                with open(ld_avis_data_filepath, 'wb') as f:
                    pickle.dump(self.LDAvis_prepared, f)
            # Load the pre-prepared pyLDAvis data from disk
            with open(ld_avis_data_filepath, 'rb') as f:
                self.LDAvis_prepared = pickle.load(f)
            pyLDAvis.save_html(self.LDAvis_prepared, 'lavs_prepared_train' + str(self.num_topics) + '.html')
        except Exception as e:
            print("No visualization was found", e)
            lg.exception(str(e))
