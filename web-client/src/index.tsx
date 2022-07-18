import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import Modal from 'react-modal';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

Modal.setAppElement(document.getElementById('root') as HTMLElement);

root.render(
  <BrowserRouter>
    <App />
  </BrowserRouter>
);
