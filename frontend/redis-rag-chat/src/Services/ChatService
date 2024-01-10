
const ChatService = () =>{
    const SendMessage = async function(message){
        try{
            return await fetch(`${process.env.REACT_APP_BACKEND_URI}/chat`,{
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    prompt:message
                })
            })
        }
        catch(e){
            console.log(e);
            return e;
        }
                
    }
}

export default ChatService;