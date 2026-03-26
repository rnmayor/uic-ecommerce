import { Button } from './button';

import type { Meta, StoryObj } from '@storybook/react';

const meta: Meta<typeof Button> = {
  title: 'Components/Button',
  component: Button,
  tags: ['autodocs'],
};

export default meta;

type Story = StoryObj<typeof Button>;

export const Primary: Story = {
  args: {
    children: 'Click Me',
    variant: 'default',
  },
};

export const Secondary: Story = {
  args: {
    children: 'Click Me',
    variant: 'destructive',
  },
};
