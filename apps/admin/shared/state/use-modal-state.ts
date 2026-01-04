import { useAppState } from './use-app-state';

import type { AdminModal } from '@shared/types/modal';

export const useModalState = (modalType: AdminModal) => {
  const isOpen = useAppState((s) => s.activeModal === modalType);
  const openModal = useAppState((s) => s.openModal);
  const closeModal = useAppState((s) => s.closeModal);

  return {
    isOpen,
    openModal: () => openModal(modalType),
    closeModal,
  };
};
