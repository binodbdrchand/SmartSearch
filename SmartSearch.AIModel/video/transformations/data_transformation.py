from gensim import corpora
from file_logger import get_logger
from config import model_config
import json
lg = get_logger()


def doc_term_matrix(bi_gram_data):
    """
    This function will transform the text data into numerical data using Gensim
    :param bi_gram_data: Bi_gram data
    :return:
    """
    try:
        corpus_list = [j for j in bi_gram_data]

        # final corpus of all the documents where each token length is greater than 2
        final_corpus_spacy = [[text_data_spacy for text_data_spacy in unique_token_list if len(text_data_spacy) > 2]
                              for unique_token_list in corpus_list]
        # for Spacy,Creating the dictionary of all the words in corpus where each unique term has assigned an index.
        dictionary_term = corpora.Dictionary(final_corpus_spacy)
        # Filter out some extremely repeating words from the documents to get unique words
        if len(final_corpus_spacy) > 4:
            with open(model_config.PARAM_FILE_NAME, 'r') as file:
                params = json.load(file)
            #dictionary_term.filter_extremes(no_below=3, no_above=.7)
            dictionary_term.filter_extremes(no_below=params['no_below'], no_above=params['no_above'])
        else:
            dictionary_term

        # save the dictionary  to disk
        # saved_dictionary.pickle_file(dictionary_term)
        # list of unique tokens for the model
        # tokens_list.unique_tokens(dictionary_term)

        # Converting list of documents into Document Term Matrix
        document_term_matrix = [dictionary_term.doc2bow(i) for i in final_corpus_spacy]
        lg.info("total length of unique words:%s", len(dictionary_term))
        lg.info("total length of BOW list :%s", len(document_term_matrix))
        lg.info("total length of final text list :%s", len(final_corpus_spacy))
        return dictionary_term, document_term_matrix, final_corpus_spacy
    except Exception as e:
        lg.exception((str(e)))
