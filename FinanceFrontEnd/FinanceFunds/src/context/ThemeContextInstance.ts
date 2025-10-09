import { createContext } from 'react';

export interface ThemeContextType {
  colorScheme: 'light' | 'dark';
  toggleColorScheme: () => void;
}

export const ThemeContext = createContext<ThemeContextType | undefined>(undefined);
