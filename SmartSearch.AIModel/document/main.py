import logging as lg
import sys

from multiprocessing import freeze_support

from file_logger import get_logger
from models import topic_model
from transformations import data_transformation
from utils import file_handling, preprocessing

lg = get_logger()


def main():
    try:
        file = [sys.argv[1]]
        text_data, files_name, file_name_only, list_text, list_page, list_doc = file_handling.file_path(file)

        lg.info("total no .of files: %s", len(files_name))
        lg.info(" file name only: %s", file_name_only)

        preprocessed_data, total_language = preprocessing.preprocessing(text_data)
        ngram_data = preprocessing.ngram_data(preprocessed_data)
        dictionary, bag_of_words, multiple_token_list = data_transformation.doc_term_matrix(ngram_data)

        lg.info('Data is fitting to Model!!')
        model = topic_model.TopicModel(
            dictionary, bag_of_words, multiple_token_list,
            preprocessed_data, files_name, total_language,
            file_name_only, list_page, list_text, list_doc)

        # Predict new files
        lg.info("Model is predicting new pdf files")
        prediction = model.model_prediction()
        lg.info("Prediction work is done !!")

        # Extracting the dominant topic from each document with necessary features
        dominant_topic = model.get_dominant_topics()

        return dominant_topic
    except Exception as e:
        print("Path is Invalid.Please Try Again!! ", e)
        lg.exception(str(e))


if __name__ == '__main__':
    freeze_support()
    json_response = main()
    print(json_response[0])
