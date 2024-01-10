import React, {Component, useEffect, useState, usestate} from 'react';
import {ChatBubble} from './ChatBubble';
import { SendMessage, StartChat } from '../api';

export class ChatWindow extends Component{
  static displayName = ChatWindow.name;
  constructor(props){
    super(props)
    this.state = {messages:[], input:'', chatId:'', awaitingServer: true}    
  }

  async componentDidMount(){    
    await this.start();
  }

  sendMessage = async () => {
    if(this.state.input.trim() !== ''){      
      this.setState({messages:[...this.state.messages, {message:this.state.input, userType:'user'}]});      
      this.setState({input:'', messagePending: true});
      const response = await SendMessage(this.state.input, this.state.chatId);
      console.log(response);
      this.setState({messagePending:false});
      this.setState({messages:[...this.state.messages, {message:response.message, userType:'bot'}]})
    }    
  }

  start = async () => {
    try{
      const response = await StartChat();
      const msg = {message:response.message, userType: 'bot'};
      this.setState({messages:[msg], input:'', chatId: response.chatId, messagePending: false, awaitingServer: false})
    }
    catch(e){
      this.setState({messages:[], input:'', chatId:'', messagePending:false, awaitingServer: true})
      setTimeout(this.start, 5000);
    }    
  }

  handleKeyPress = (event)=>{
    if (event.key === 'Enter') {
      this.sendMessage();
    }
  };

  render(){
    if(this.state.awaitingServer){
      return(
        <div style={{display:'flex', justifyContent:'center', alignItems:'center', height: '100vh'}}>
          <span>Awaiting Server to start up on {process.env.REACT_APP_BACKEND_URI}...</span>
        </div>
      )
    }

    return(
    <div style={{display: 'flex', flexDirection: 'column', height: '100vh', width: '100vw', backgroundColor: 'darkgray'}}>
      <div style={{ flexGrow:1, overflowY: 'auto', padding: '10px', display: 'flex', flexDirection: 'column' }}>
        {this.state.messages.map((msg, index) => (
            <ChatBubble message={msg.message} userType={msg.userType}/>
        ))}
        {
          this.state.messagePending ?
            (<ChatBubble message="awaiting response" userType="system"/>):
            (
              <div/>
            )   
          
        }
      </div>
      <div style={{display: 'flex', justifyContent: 'center', marginBottom: '10px'}}>
        <textarea
            disabled={this.state.messagePending?'disabled':''}
            type="text"            
            value={this.state.input}
            onChange={(e) => this.setState({input:e.target.value})}
            onKeyUp={this.handleKeyPress}
            rows={3}
            cols={30}
            style={{margin: '5px', flexGrow: 1, maxWidth: '600px', borderRadius: '10px', overflowWrap: 'break-word', overflowX: 'hidden', overflowY: 'auto'}}
        />
        <button disabled={this.state.messagePending?'disabled':''} onClick={this.sendMessage}>Send</button>            
      </div>          
    </div>
    );
  }
}