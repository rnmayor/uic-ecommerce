'use client';

import { Half2Icon, MoonIcon, SunIcon } from '@radix-ui/react-icons';
import { useTheme } from 'next-themes';
import { useEffect, useState } from 'react';

import { Button } from '../ui/button';

export const ThemeToggler = () => {
  const { theme, setTheme } = useTheme();
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setMounted(true);
  }, []);

  if (!mounted) {
    return (
      <Button
        variant="outline"
        size="icon-sm"
        className="relative transition-opacity duration-200 opacity-80"
      >
        <Half2Icon className="animate-pulse" />
      </Button>
    );
  }

  const toggleTheme = () => setTheme(theme === 'dark' ? 'light' : 'dark');

  return (
    <Button
      variant="outline"
      size="icon-sm"
      className="relative transition-all duration-100 hover:text-primary/90 group/button"
      aria-label={theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'}
      onClick={toggleTheme}
    >
      <SunIcon className="rotate-0 opacity-100 scale-100 transition-all dark:opacity-0 dark:scale-0 group-hover/button:scale-110" />
      <MoonIcon className="absolute rotate-90 opacity-0 scale-0 transition-all dark:rotate-0 dark:opacity-100 dark:scale-100 group-hover/button:dark:scale-115" />
    </Button>
  );
};
