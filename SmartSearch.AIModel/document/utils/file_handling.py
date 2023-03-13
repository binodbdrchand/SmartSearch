import logging as lg
import fitz

from file_logger import get_logger

lg = get_logger()


def file_path(file_list):
    """
        This function helps to read  the files stored in the system,saves the filename with  its location.
        :param file_list: list of Path of the file location
        :return: list of files
        """
    try:
        total_files = [file_name for file_name in file_list if '.pdf' in file_name]

        lg.info("total file list %s ", total_files)

        temp_docs = []
        file_name_only = []

        list_text = []
        list_page = []
        list_doc = []

        for file in total_files:

            file_name_as_string = ""
            file_name_as_string = file_name_as_string.join(file)
            last_index_of = file_name_as_string.rfind('/')
            total_length = len(file_name_as_string)
            file_naming = file_name_as_string[last_index_of + 1:total_length]
            file_name_only.append(file_naming)

            with fitz.open(file) as pdf_doc:
                text = ""
                for page in pdf_doc:
                    page_text = ''
                    page_text += page.getText()
                    list_text.append(page_text.lower())
                    list_page.append(page.number + 1)
                    list_doc.append(file_naming)
                    text += page.getText()

                temp_docs.append(text)

        return temp_docs, total_files, file_name_only, list_page, list_text, list_doc
    except Exception as e:
        print("Required path is not found", e)
        lg.exception((str(e)))


