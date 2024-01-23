from io import BytesIO

import PyPDF2
from fastapi import UploadFile


async def extract_text_from_upload(upload_file: UploadFile) -> str:
    file_stream = await upload_file.read()

    if upload_file.content_type == 'application/pdf':
        file_obj = BytesIO(file_stream)
        reader = PyPDF2.PdfReader(file_obj)
        text = ""
        for page in reader.pages:
            text += page.extract_text()

        return text
    else:
        return file_stream.decode('utf-8')
