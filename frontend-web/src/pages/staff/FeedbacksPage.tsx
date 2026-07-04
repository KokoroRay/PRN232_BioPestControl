import React, { useEffect, useState } from 'react';
import axios from 'axios';

interface Feedback {
  id: string;
  userName: string;
  productId: number;
  rating: number;
  comment: string;
  createdAt: string;
  replyMessage?: string;
  repliedAt?: string;
}

const FeedbacksPage: React.FC = () => {
  const [feedbacks, setFeedbacks] = useState<Feedback[]>([]);
  const [loading, setLoading] = useState(true);
  const [replyText, setReplyText] = useState('');
  const [replyingTo, setReplyingTo] = useState<string | null>(null);

  useEffect(() => {
    fetchFeedbacks();
  }, []);

  const fetchFeedbacks = async () => {
    try {
      const { data } = await axios.get('http://localhost:5000/api/feedbacks');
      setFeedbacks(data);
    } catch (err) {
      console.error('Failed to load feedbacks', err);
    } finally {
      setLoading(false);
    }
  };

  const submitReply = async (feedbackId: string) => {
    try {
      await axios.post(`http://localhost:5000/api/feedbacks/${feedbackId}/reply`, {
        replyMessage: replyText,
        staffId: '00000000-0000-0000-0000-000000000000' // mock staff id
      });
      setReplyText('');
      setReplyingTo(null);
      fetchFeedbacks();
    } catch (err) {
      console.error('Failed to submit reply', err);
    }
  };

  if (loading) return <div className="p-6">Loading...</div>;

  return (
    <div className="p-6 bg-surface-container-lowest min-h-[calc(100vh-64px)]">
      <div className="mb-6">
        <h2 className="text-2xl font-bold text-primary">Feedback Management</h2>
        <p className="text-on-surface-variant">Review customer feedback and respond</p>
      </div>

      <div className="bg-white dark:bg-surface rounded-2xl shadow-sm border border-outline-variant/20 overflow-hidden">
        <table className="w-full text-left border-collapse">
          <thead>
            <tr className="bg-surface-variant/30 border-b border-outline-variant/20">
              <th className="p-4 font-semibold text-primary">Customer</th>
              <th className="p-4 font-semibold text-primary">Product ID</th>
              <th className="p-4 font-semibold text-primary">Rating</th>
              <th className="p-4 font-semibold text-primary">Comment</th>
              <th className="p-4 font-semibold text-primary">Date</th>
              <th className="p-4 font-semibold text-primary">Status</th>
              <th className="p-4 font-semibold text-primary">Action</th>
            </tr>
          </thead>
          <tbody>
            {feedbacks.map(fb => (
              <tr key={fb.id} className="border-b border-outline-variant/10 hover:bg-surface-variant/10">
                <td className="p-4 text-sm font-medium">{fb.userName}</td>
                <td className="p-4 text-sm">{fb.productId}</td>
                <td className="p-4 text-sm">
                  <span className="flex text-amber-500">
                    {'★'.repeat(fb.rating)}{'☆'.repeat(5 - fb.rating)}
                  </span>
                </td>
                <td className="p-4 text-sm max-w-xs truncate">{fb.comment}</td>
                <td className="p-4 text-sm">{new Date(fb.createdAt).toLocaleDateString()}</td>
                <td className="p-4 text-sm">
                  {fb.replyMessage ? (
                    <span className="px-2 py-1 bg-green-100 text-green-700 rounded text-xs">Replied</span>
                  ) : (
                    <span className="px-2 py-1 bg-orange-100 text-orange-700 rounded text-xs">Pending</span>
                  )}
                </td>
                <td className="p-4">
                  {!fb.replyMessage && replyingTo !== fb.id && (
                    <button onClick={() => setReplyingTo(fb.id)} className="text-primary hover:underline text-sm font-medium">Reply</button>
                  )}
                  {replyingTo === fb.id && (
                    <div className="flex flex-col gap-2">
                      <textarea 
                        className="w-full border rounded p-2 text-sm" 
                        rows={2} 
                        placeholder="Write a reply..."
                        value={replyText}
                        onChange={e => setReplyText(e.target.value)}
                      />
                      <div className="flex gap-2">
                        <button onClick={() => submitReply(fb.id)} className="bg-primary text-white px-3 py-1 rounded text-xs">Send</button>
                        <button onClick={() => setReplyingTo(null)} className="bg-gray-200 px-3 py-1 rounded text-xs text-black">Cancel</button>
                      </div>
                    </div>
                  )}
                  {fb.replyMessage && (
                    <p className="text-xs text-gray-500 max-w-xs truncate" title={fb.replyMessage}>Reply: {fb.replyMessage}</p>
                  )}
                </td>
              </tr>
            ))}
            {feedbacks.length === 0 && (
              <tr>
                <td colSpan={7} className="p-8 text-center text-on-surface-variant">No feedback found</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default FeedbacksPage;
