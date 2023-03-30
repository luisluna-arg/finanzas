import React from "react";

const stateContext = React.createContext();

export const Provider = props => {
    const [context, setContext] = React.useState({});

    return (
        <stateContext.Provider value={{ context, setContext }}>
            {props.children}
        </stateContext.Provider>
    );
};

export const useStateContext = () => {
    return React.useContext(stateContext);
};
