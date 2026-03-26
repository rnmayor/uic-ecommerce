import { Button } from './button';

import type { Meta, StoryObj } from '@storybook/react';

const meta: Meta<typeof Button> = {
  title: 'COMPONENTS/UI/Button',
  component: Button,
};

export default meta;

type Story = StoryObj<typeof Button>;

export const Primary: Story = {
  args: {
    children: 'Click Me...',
    variant: 'default',
  },
};

export const Secondary: Story = {
  args: {
    children: 'Click Me...',
    variant: 'secondary',
  },
};

export const Outline: Story = {
  args: {
    children: 'Click Me...',
    variant: 'outline',
  },
};

export const Ghost: Story = {
  args: {
    children: 'Click Me...',
    variant: 'ghost',
  },
};

export const Destructive: Story = {
  args: {
    children: 'Click Me...',
    variant: 'destructive',
  },
};

export const Link: Story = {
  args: {
    children: 'Click Me...',
    variant: 'link',
  },
};
