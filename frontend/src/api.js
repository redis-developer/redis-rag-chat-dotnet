export const SendMessage = async function(message, chatId){
    const responseMessage = await fetch(`${process.env.REACT_APP_BACKEND_URI}/chat/${chatId}`,{
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            prompt:message
        })
    });

    return responseMessage.json();
}

export const StartChat = async function(){
    const responseMessage = await fetch(`${process.env.REACT_APP_BACKEND_URI}/chat/startChat`, {
        headers: {
            'Content-Type': 'application/json'
        },
        method: 'POST'    
    })       
    return responseMessage.json();
}

export const UploadDocument = async function(file){
    const form = new FormData();
    form.append('file', file);
    const response = await fetch(`${process.env.REACT_APP_BACKEND_URI}/documents/upload`,
        {
            method:'POST',
            body: form
        })

        if(response.status === 200){
            return "Upload Completed Successfully";
        }

        return response.statusText;
}