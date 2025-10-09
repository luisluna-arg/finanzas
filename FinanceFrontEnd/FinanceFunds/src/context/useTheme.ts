import { useContext } from 'react';
import { ThemeContext } from './ThemeContextInstance';
import type { ThemeContextType } from './ThemeContextInstance';

export function useTheme(): ThemeContextType {
  const context = useContext(ThemeContext);
  if (context === undefined) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  return context;
}
