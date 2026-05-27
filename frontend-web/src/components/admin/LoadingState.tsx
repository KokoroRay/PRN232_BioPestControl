import React from 'react';

export const LoadingState: React.FC<{ message?: string }> = ({ message = 'Loading...' }) => (
  <div className="loading-state">
    <div className="spinner" />
    <p>{message}</p>
  </div>
);
