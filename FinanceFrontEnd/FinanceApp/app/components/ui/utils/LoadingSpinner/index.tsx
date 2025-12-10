import React from 'react';
import styles from './index.module.scss';

interface LoadingSpinnerProps {
    size?: number; // Size of the spinner in pixels
    color?: string; // Color of the spinner
}

const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({ size = 40, color = "#3498db" }) => {
    return (
        <div
            className={styles.spinner}
            style={{
                // Pass size and color as CSS variables
                '--spinner-size': `${size}px`,
                '--spinner-color': color,
            } as React.CSSProperties} // Type assertion to handle CSS variables
        ></div>
    );
};

export default LoadingSpinner;
