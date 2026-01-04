import { createModalState, type ModalState } from '@ecommerce/core';
import { create } from 'zustand';
import { devtools } from 'zustand/middleware';

import type { AdminModal } from '@shared/types/modal';

type AppState = ModalState<AdminModal>;

export const useAppState = create<AppState>()(
  devtools(
    (set, get, api) => ({
      // combine all slice states and actions
      ...createModalState<AdminModal>()(set, get, api),
    }),
    { name: 'UIC Admin App State' }, // label in Redux DevTools
  ),
);
