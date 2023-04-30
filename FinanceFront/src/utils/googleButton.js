import React, { useState, useEffect } from 'react';
import { GoogleLogin } from '@react-oauth/google'
import { loginSuccess, checkAutoLogin } from './../services/AuthService'

const GoogleButton = function () {
  let [loggedIn, setLoggedIn] = useState(false);

  useEffect(() => {
    setLoggedIn(checkAutoLogin());
  }, [setLoggedIn]);

  if (!loggedIn) {
    return (
      <GoogleLogin
        onSuccess={loginSuccess}
        onError={() => {
          console.log('Login Failed');
        }}
      />
    );
  }
}

export default GoogleButton;
