import React, { useState, useRef, useEffect } from 'react';
import { Bot, X, Send, Image as ImageIcon, MessageSquare, Loader2 } from 'lucide-react';
import { aiService } from '../../services/aiService';
import { CameraCapture } from './CameraCapture';

type Mode = 'chat' | 'disease';

interface Message {
  id: string;
  role: 'user' | 'assistant';
  content: string;
  images?: string[];
}

export const AIAssistantWidget: React.FC = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [mode, setMode] = useState<Mode>('chat');
  const [messages, setMessages] = useState<Message[]>([
    { id: '1', role: 'assistant', content: 'Hello! I am the BioPestControl AI. How can I help you today?' }
  ]);
  const [input, setInput] = useState('');
  const [stagedImages, setStagedImages] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  const [showCamera, setShowCamera] = useState(false);
  
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages, stagedImages]);

  const toggleWidget = () => {
    if (isOpen) {
      setShowCamera(false);
      setMode('chat');
    }
    setIsOpen(!isOpen);
  };

  const handleSendText = async () => {
    if ((!input.trim() && stagedImages.length === 0) || loading) return;

    const userMessage: Message = { 
      id: Date.now().toString(), 
      role: 'user', 
      content: input,
      images: [...stagedImages]
    };
    
    setMessages(prev => [...prev, userMessage]);
    
    const textToSend = input;
    const imagesToSend = [...stagedImages];
    
    setInput('');
    setStagedImages([]);
    setLoading(true);

    try {
      const result = await aiService.chat(textToSend, imagesToSend);
      const botMsg: Message = { 
        id: (Date.now() + 1).toString(), 
        role: 'assistant', 
        content: result.success ? result.response : (result.errorMessage || 'Sorry, I encountered an error.') 
      };
      setMessages(prev => [...prev, botMsg]);
    } catch (error) {
      setMessages(prev => [...prev, { id: Date.now().toString(), role: 'assistant', content: 'Connection error.' }]);
    } finally {
      setLoading(false);
    }
  };

  const handleImageCapture = (base64Image: string) => {
    setStagedImages(prev => [...prev, base64Image]);
    setShowCamera(false);
    setMode('chat');
  };

  const handleFileUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files || []);
    if (files.length === 0) return;

    files.forEach(file => {
      const reader = new FileReader();
      reader.onload = (event) => {
        const base64 = event.target?.result as string;
        setStagedImages(prev => [...prev, base64]);
      };
      reader.readAsDataURL(file);
    });
    // Reset input
    e.target.value = '';
  };

  const removeStagedImage = (index: number) => {
    setStagedImages(prev => prev.filter((_, i) => i !== index));
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
          onClick={() => { setMode('chat'); setShowCamera(false); }}
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
                  {msg.images && msg.images.length > 0 && (
                    <div className="ai-message-images">
                      {msg.images.map((img, idx) => (
                        <img key={idx} src={img} alt={`Attached ${idx}`} className="ai-uploaded-img" />
                      ))}
                    </div>
                  )}
                  {msg.content && <p>{msg.content}</p>}
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
        <div className="ai-widget-footer-wrapper">
          {stagedImages.length > 0 && (
            <div className="ai-staged-images">
              {stagedImages.map((img, idx) => (
                <div key={idx} className="ai-staged-img-container">
                  <img src={img} alt={`Staged ${idx}`} />
                  <button className="ai-staged-remove" onClick={() => removeStagedImage(idx)}>
                    <X size={12} />
                  </button>
                </div>
              ))}
            </div>
          )}
          <div className="ai-widget-footer">
            <label className="ai-upload-btn" style={{ cursor: 'pointer', padding: '0 8px', color: '#666' }}>
              <ImageIcon size={18} />
              <input type="file" accept="image/*" multiple style={{ display: 'none' }} onChange={handleFileUpload} />
            </label>
            <input 
              type="text" 
              placeholder={mode === 'chat' ? "Ask about BioPestControl..." : "Upload or describe..."} 
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && handleSendText()}
              disabled={loading}
            />
            <button className="ai-send-btn" onClick={handleSendText} disabled={loading || (!input.trim() && stagedImages.length === 0)}>
              <Send size={18} />
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
