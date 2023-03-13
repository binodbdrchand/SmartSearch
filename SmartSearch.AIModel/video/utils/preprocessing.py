import os.path

import de_core_news_lg
import en_core_web_lg
import json
import nltk
import pandas as pd
import re
import spacy
import string

from gensim.models.phrases import Phrases, Phraser
from langdetect import detect
from nltk.corpus import stopwords

from config import model_config
from file_logger import get_logger


lg = get_logger()
nlp_german = de_core_news_lg.load()

with open(os.path.dirname(__file__) + '../../config/params.json', 'r') as file:
    params = json.load(file)


def preprocessing(final_data):
    """
    This function helps to preprocessed the text data from different files and extract all the words and store list .
    :param final_data: final data extracted from all thee video files.
    :return: list of corpus
    """
    try:
        exclude = set(string.punctuation)
        video_corpus = []
        total_language = []
        for i in range(len(final_data)):
            # Removing unnecessary symbols
            review = re.sub('[^a-zA-ZäöüÄÖÜß0-9]', ' ', final_data[i])

            # To detect the language
            language = detect(review)
            # Convert text into lower case and remove the stop words
            stop_free_word = [i for i in review.lower().split() if i not in set(stopwords.words('german'))]

            # Remove any stop words presents
            punctuation_free_word = nlp_german(' '.join([word for word in stop_free_word if word not in exclude]))
            # Extract the root word using lemmantization and placed in the list
            pos_word_lemma = (' '.join([token.lemma_ for token in punctuation_free_word
                                       if (token.pos_ == 'NOUN' or token.pos_ == 'PROPN' or token.pos_ == 'VERB')]))
            video_corpus.append(pos_word_lemma)
            total_language.append(language)
        lg.info("Preprocessing work is completed !!")

        return video_corpus, total_language
    except Exception as e:
        lg.exception(str(e))


def bi_gram_data(preprocess_data):
    """
    The function generates the bi gram words from each text data using gensim
    and performs the filter operation for most repetitive  word.
    :param preprocess_data: list of preprocessed data
    :return: bi gram data
    """
    try:
        data_frame = pd.DataFrame(preprocess_data, columns=['Documents'])
        pd.options.display.max_colwidth = 100
        row_document = [row.split() for row in data_frame['Documents']]

        # Perform bi-gram filter operation Higher the threshold value,lesser the phrases
        phrases = Phrases(row_document, min_count=params['min_count'], threshold=params['threshold'], delimiter=' ')
        bi_gram = Phraser(phrases)
        sentences_video = bi_gram[row_document]

        return sentences_video
    except Exception as e:
        lg.exception(str(e))
