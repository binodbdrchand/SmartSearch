import de_core_news_lg
import en_core_web_lg
import logging as lg
import re
import string

import pandas as pd

from gensim.models.phrases import Phrases, Phraser
from langdetect import detect
from nltk.corpus import stopwords

from file_logger import get_logger


lg = get_logger()
nlp = de_core_news_lg.load()
nlp_eng = en_core_web_lg.load()
stopwords_English = stopwords.words('english')
stopwords_German = stopwords.words('german')
stopwords_German.extend(["änderungsdienst", "arbeitsanweisung", "seite",
                         "verfahrensanweisung", "änderung", "ausdruck",
                         "unterliegen", "unterliegt", "änderungen", "ändern"])


def preprocessing(text_data):
    """
    This function helps to preprocessed the text data from different files and
    extract all the words and store list .
    :param text_data:
    :return: list of corpus
    """
    try:
        exclude = set(string.punctuation)
        corpus_spacy = []
        total_language = []

        for i in range(len(text_data)):
            # detect the language of text data
            text_lang = re.sub('[^a-zA-Z0-9]', ' ', text_data[i])
            language = detect(text_lang)
            if language == "en":
                # Removing unnecessary symbols
                review_spacy = re.sub('[^a-zA-Z0-9]', ' ', text_data[i])

                # Convert text into lower case and split into words
                stopfree_word_spacy = [i for i in review_spacy.lower().split() if
                                       i not in set(stopwords_English)]

                # Remove any stop word presents
                puncfree_word = nlp_eng(' '.join([word for word in stopfree_word_spacy if word not in exclude]))

                # Extract the root word using lemmantization and placed in list.
                root_word_spacy = (' '.join([token.lemma_ for token in puncfree_word if
                                   (token.pos_ == 'NOUN' or token.pos_ == "PROPN" or token.pos_ == 'VERB')]))
            else:
                # Removing unnecessary symbols
                review_spacy = re.sub('[^a-zA-ZäöüÄÖÜß0-9]', ' ', text_data[i])

                # convert text into lower case and split into words
                stopfree_word_spacy = [i for i in review_spacy.lower().split() if
                                       i not in set(stopwords_German)]

                # Remove any stop word presents
                puncfree_word = nlp(' '.join([word for word in stopfree_word_spacy if word not in exclude]))

                # Extract the root word using lemmantization and placed in list.
                root_word_spacy = (' '.join([token.lemma_ for token in puncfree_word if
                                   (token.pos_ == 'NOUN' or token.pos_ == "PROPN" or token.pos_ == 'VERB')]))
            corpus_spacy.append(root_word_spacy)
            total_language.append(language)

        lg.info("Preprocessing work is completed !!")

        return corpus_spacy, total_language
    except Exception as e:
        print("Text data is Invalid.Please Try Again!!", e)
        lg.exception((str(e)))


def ngram_data(preprocessed_data):
    """
    The function generates the bigram words from each text data using gensim
    and performs the filter operation for most repetitive  word.

    :param preprocessed_data:
    :return:
    """
    try:
        data_frame = pd.DataFrame(preprocessed_data, columns=['Documents'])
        pd.options.display.max_colwidth = 100
        row_document = [row.split() for row in data_frame['Documents']]

        # Perform  bi-gram filter operation Higher the threshold value,lesser the phrases
        phrases = Phrases(row_document, min_count=5, threshold=60, delimiter=' ')
        bigram_spacy = Phraser(phrases)
        sentences_spacy = bigram_spacy[row_document]
        return sentences_spacy
    except Exception as e:
        print("Invalid text data .Please Try again !!", e)
        lg.exception((str(e)))
