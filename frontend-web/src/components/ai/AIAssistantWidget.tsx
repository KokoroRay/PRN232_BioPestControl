import React, { useState, useRef, useEffect } from 'react';
import { Bot, X, Send, Image as ImageIcon, MessageSquare, Loader2 } from 'lucide-react';
import { aiService } from '../../services/aiService';
import { CameraCapture } from './CameraCapture';

type Mode = 'chat' | 'disease';

interface Message {
  id: string;
  role: 'user' | 'assistant';
  content: string;
  isImage?: boolean;
}

export const AIAssistantWidget: React.FC = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [mode, setMode] = useState<Mode>('chat');
  const [messages, setMessages] = useState<Message[]>([
    { id: '1', role: 'assistant', content: 'Hello! I am the BioPestControl AI. How can I help you today?' }
  ]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(false);
  const [showCamera, setShowCamera] = useState(false);
  
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const toggleWidget = () => setIsOpen(!isOpen);

  const handleSendText = async () => {
    if (!input.trim() || loading) return;

    const userMessage: Message = { id: Date.now().toString(), role: 'user', content: input };
    setMessages(prev => [...prev, userMessage]);
    setInput('');
    setLoading(true);

    try {
      const result = await aiService.chat(userMessage.content);
      const botMsg: Message = { 
        id: (Date.now() + 1).toString(), 
        role: 'assistant', 
        content: result.success ? result.response : 'Sorry, I encountered an error.' 
      };
      setMessages(prev => [...prev, botMsg]);
    } catch (error) {
      setMessages(prev => [...prev, { id: Date.now().toString(), role: 'assistant', content: 'Connection error.' }]);
    } finally {
      setLoading(false);
    }
  };

  const handleImageCapture = async (base64Image: string) => {
    setShowCamera(false);
    const userMessage: Message = { id: Date.now().toString(), role: 'user', content: base64Image, isImage: true };
    setMessages(prev => [...prev, userMessage]);
    setLoading(true);

    try {
      const result = await aiService.analyzeDisease(base64Image);
      const botMsg: Message = { 
        id: (Date.now() + 1).toString(), 
        role: 'assistant', 
        content: result.success ? result.response : 'Sorry, I could not analyze the image.' 
      };
      setMessages(prev => [...prev, botMsg]);
    } catch (error) {
      setMessages(prev => [...prev, { id: Date.now().toString(), role: 'assistant', content: 'Connection error.' }]);
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) {
    return (
      <button className="ai-widget-toggle" onClick={toggleWidget} aria-label="Open AI Assistant">
        <Bot size={24} />
      </button>
    );
  }

  return (
    <div className="ai-widget-container">
      <div className="ai-widget-header">
        <div className="ai-widget-title">
          <Bot size={20} />
          <span>AI Assistant</span>
        </div>
        <button className="ai-widget-close" onClick={toggleWidget}>
          <X size={20} />
        </button>
      </div>

      <div className="ai-widget-tabs">
        <button 
          className={`ai-tab ${mode === 'chat' ? 'active' : ''}`} 
          onClick={() => setMode('chat')}
        >
          <MessageSquare size={16} /> Chat
        </button>
        <button 
          className={`ai-tab ${mode === 'disease' ? 'active' : ''}`} 
          onClick={() => { setMode('disease'); setShowCamera(true); }}
        >
          <ImageIcon size={16} /> Diagnose
        </button>
      </div>

      <div className="ai-widget-body">
        {mode === 'disease' && showCamera ? (
          <CameraCapture 
            onCapture={handleImageCapture} 
            onCancel={() => { setShowCamera(false); setMode('chat'); }} 
          />
        ) : (
          <div className="ai-messages">
            {messages.map(msg => (
              <div key={msg.id} className={`ai-message-row ${msg.role}`}>
                <div className="ai-message-bubble">
                  {msg.isImage ? (
                    <img src={msg.content} alt="User upload" className="ai-uploaded-img" />
                  ) : (
                    <p>{msg.content}</p>
                  )}
                </div>
              </div>
            ))}
            {loading && (
              <div className="ai-message-row assistant">
                <div className="ai-message-bubble loading">
                  <Loader2 size={16} className="spin" /> Thinking...
                </div>
              </div>
            )}
            <div ref={messagesEndRef} />
          </div>
        )}
      </div>

      {(!showCamera || mode === 'chat') && (
        <div className="ai-widget-footer">
          <input 
            type="text" 
            placeholder={mode === 'chat' ? "Ask about BioPestControl..." : "Upload or describe..."} 
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleSendText()}
            disabled={loading}
          />
          <button className="ai-send-btn" onClick={handleSendText} disabled={loading || !input.trim()}>
            <Send size={18} />
          </button>
        </div>
      )}
    </div>
  );
};
