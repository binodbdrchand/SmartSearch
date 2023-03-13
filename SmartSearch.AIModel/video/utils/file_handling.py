from file_logger import get_logger
lg = get_logger()


def file_path(path):

    """pip
    This function helps to read  the .mp4 files stored in the system,saves the filename with  its location.
    :param path: Path of the file location
    :return: list of files
    """
    try:
        '''
        (os.chdir(path))
        total_files = [file_name for file_name in os.listdir() if
                       '.mp4' in file_name and (os.path.isfile(file_name) == True)]
        '''
        # logger.log("total file list %s ",total_files)
        total_files = [file_name for file_name in path if
                       '.mp4' in file_name]
        file_name_only = []
        for file in total_files:
            # Extracting the exact file name only
            file_name_as_string = ""
            file_name_as_string = file_name_as_string.join(file)
            last_index_of = file_name_as_string.rfind('/')
            total_length = len(file_name_as_string)
            file_naming = file_name_as_string[last_index_of + 1:total_length]
            file_name_only.append(file_naming)
        lg.info("total file list %s ", total_files)
        return total_files, file_name_only
    except Exception as e:
        # print("Required file path is not found", e)
        lg.exception((str(e)))
