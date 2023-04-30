import React from 'react';
// import logo from './logo.svg';
import './index.scss';
import './mui.scss';
import './popup.scss';
import { GoogleOAuthProvider } from '@react-oauth/google';
import Login from './../../components/Login';
import Home from './../../components/Home';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <GoogleOAuthProvider clientId={process.env.REACT_APP_GOOGLE_OAUTH_CLIENT_ID}>
          <Login />
          <Home />
        </GoogleOAuthProvider>
      </header>
    </div>
  );
}

export default App;
