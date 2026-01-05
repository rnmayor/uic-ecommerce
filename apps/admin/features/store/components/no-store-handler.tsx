'use client';

import { useEffect } from 'react';

import { useModalState } from '@shared/state/use-modal-state';

export const NoStoreHandler = () => {
  const { isOpen, openModal } = useModalState('store');
  useEffect(() => {
    if (!isOpen) {
      openModal();
    }
  }, [isOpen, openModal]);

  return null;
};
