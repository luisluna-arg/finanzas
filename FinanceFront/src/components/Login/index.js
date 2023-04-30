import GoogleButton from './../../utils/googleButton';
import { useState, useEffect } from "react";
import { checkLoggedIn } from './../../services/AuthService';

function Login() {

    let [loggedIn, setLoggedIn] = useState(false);

    useEffect(
        () => {
            setLoggedIn(checkLoggedIn());
        }, [setLoggedIn]
    );

    if (loggedIn) return (<></>);

    return (
        <div className="d-flex justify-content-center pt-5">
            <GoogleButton />
        </div>
    );
}

export default Login;