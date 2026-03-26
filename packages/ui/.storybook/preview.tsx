import { ThemeProvider } from 'next-themes';

import type { Preview } from '@storybook/react';
import '../src/styles/globals.css';

export const globalTypes = {
  theme: {
    name: 'Theme',
    description: 'Global theme for components',
    defaultValue: 'system', // or 'light'
    toolbar: {
      icon: 'mirror',
      items: [
        { value: 'light', title: 'Light', icon: 'sun' },
        { value: 'dark', title: 'Dark', icon: 'moon' },
        { value: 'system', title: 'System', icon: 'mirror' },
      ],
    },
  },
};

const preview: Preview = {
  parameters: {
    controls: {
      expanded: true,
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/i,
      },
    },
    a11y: {
      disable: false,
    },
    backgrounds: {
      disable: true,
    },
  },
  decorators: [
    (Story, context) => {
      const theme = context.globals.theme;

      return (
        <ThemeProvider
          attribute="class"
          enableSystem={theme === 'system'}
          forcedTheme={theme === 'system' ? undefined : theme}
        >
          <div className="p-4 bg-background text-foreground min-h-screen flex items-center justify-center">
            <Story />
          </div>
        </ThemeProvider>
      );
    },
  ],
};

export default preview;
