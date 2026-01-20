'use client';

import { useEffect } from 'react';

import { useModalState } from '@shared/state/use-modal-state';

export const NoTenantHandler = () => {
  const { isOpen, openModal } = useModalState('tenant');
  useEffect(() => {
    if (!isOpen) {
      openModal();
    }
  }, [isOpen, openModal]);

  return null;
};
