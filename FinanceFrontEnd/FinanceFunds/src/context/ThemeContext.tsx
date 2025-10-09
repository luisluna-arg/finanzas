import { useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import { ThemeContext } from './ThemeContextInstance';

export function ThemeProvider({ children }: { children: ReactNode }) {
  // Get initial color scheme from localStorage or use system preference
  const getInitialColorScheme = (): 'light' | 'dark' => {
    const savedScheme = localStorage.getItem('colorScheme');
    if (savedScheme === 'light' || savedScheme === 'dark') {
      return savedScheme;
    }

    // Check system preference
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
      return 'dark';
    }

    return 'light';
  };

  const [colorScheme, setColorScheme] = useState<'light' | 'dark'>(getInitialColorScheme);

  // Toggle color scheme
  const toggleColorScheme = () => {
    setColorScheme(current => {
      const newScheme = current === 'dark' ? 'light' : 'dark';
      localStorage.setItem('colorScheme', newScheme);
      return newScheme;
    });
  };

  // Update document body class when color scheme changes
  useEffect(() => {
    document.body.classList.toggle('dark-theme', colorScheme === 'dark');
  }, [colorScheme]);

  return (
    <ThemeContext.Provider value={{ colorScheme, toggleColorScheme }}>
      {children}
    </ThemeContext.Provider>
  );
}

// useTheme hook has been moved to a separate file
