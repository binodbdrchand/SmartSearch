import logging
import os


def get_logger():
    """
    This function creates the log event of whole program and stores in logger file.
    :return: data logger
    """
    try:
        logger = logging.getLogger(__name__)
        logger.setLevel(logging.INFO)
        # create console handler and set level to debug
        # root_dir = os.path.dirname(os.path.abspath(__file__)).rsplit(os.sep, 1)[0]
        root_dir = os.path.dirname(os.path.abspath(__file__))
        filename = "videofile_handling.log"
        log_file = os.path.join(root_dir, "logs", filename)
        file_handler = logging.FileHandler(log_file)
        file_handler.setLevel(logging.INFO)
        # create formatter
        formatter = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')
        # add formatter to file_handler
        file_handler.setFormatter(formatter)
        # add file_handler to logger
        if not logger.handlers:
            logger.addHandler(file_handler)
        return logger
    except Exception as e:
        print("Invalid log data .Please Try again !!", str(e))
