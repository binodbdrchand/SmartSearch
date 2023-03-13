import logging as lg
import os

from gensim import corpora

from file_logger import get_logger


lg = get_logger()


def doc_term_matrix(ngram_data):
    """
    This function will transform the text data into numerical data using Gensim
    :param ngram_data: bi-gram data
    :return:
    """
    try:
        final_corpus_spacy = [j for j in ngram_data]
        # Final corpus of all the documents where each token length is greater than 2
        multiple_token_list = [[textdata_spacy for textdata_spacy in list if len(textdata_spacy) > 2]
                               for list in final_corpus_spacy]

        # For Spacy,Creating the dictionary of all the words in corpus where each unique term has assigned an index.
        dictionary_term = corpora.Dictionary(multiple_token_list)

        # Filter some extremely repeating words from the documents to get unique words
        if len(final_corpus_spacy) > 4:
            dictionary_term.filter_extremes(no_below=5, no_above=0.5)
        else:
            dictionary_term

        for k in dictionary_term.values():
            root_dir = os.path.dirname(os.path.abspath(__file__))
            filename = 'NLP_lemmantization_Spacy_filter.txt'
            text_file = os.path.join(root_dir, filename)
            with open(text_file, 'a') as file_NLPlemma_Spacy_filter:
                file_NLPlemma_Spacy_filter.write(k)
                file_NLPlemma_Spacy_filter.write('\n')
                file_NLPlemma_Spacy_filter.close()

        # Converting list of documents into Document Term Matrix
        document_term_matrix = [dictionary_term.doc2bow(i) for i in multiple_token_list]

        lg.info("total length of unique words:%s", len(dictionary_term))
        lg.info("total length of BOW list :%s", len(document_term_matrix))
        lg.info("total length of final text list :%s", len(multiple_token_list))

        return dictionary_term, document_term_matrix, multiple_token_list
    except Exception as e:
        print("Invalid text data ", e)
        lg.exception((str(e)))




