/* eslint-disable react/prop-types */
import React from "react";

type ContainerProps = {
    children?: React.ReactNode;
};

export const Container: React.FC<ContainerProps> = ({ children }) => {
    return <>{children}</>;
};
