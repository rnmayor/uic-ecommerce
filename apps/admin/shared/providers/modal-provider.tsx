'use client';

import { useMounted } from '@ecommerce/core';

import { StoreModal } from '@features/store/components/store-modal';
import { useAppState } from '@shared/state/use-app-state';

import type { AdminModal } from '@shared/types/modal';

// Import all modals that might appear
const modalMap: Record<AdminModal, React.ComponentType> = {
  store: StoreModal,
};

export function ModalProvider() {
  const mounted = useMounted();
  const activeModal = useAppState((s) => s.activeModal);

  if (!mounted || !activeModal) return null;

  const ModalComponent = modalMap[activeModal];

  return <ModalComponent />;
}
