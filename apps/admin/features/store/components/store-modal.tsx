'use client';

import { Button, Modal } from '@ecommerce/ui';

import { useModalState } from '@shared/state/use-modal-state';

export const StoreModal = () => {
  const { isOpen, closeModal } = useModalState('store');
  // TODO: state form logic here
  return (
    <Modal
      title="Create Store"
      description="Add new store to manage your products"
      isOpen={isOpen}
      onClose={closeModal}
      footer={
        <>
          <Button type="button" variant="outline" onClick={closeModal}>
            Cancel
          </Button>
          <Button type="submit">Continue</Button>
        </>
      }
    >
      TODO: Form here
    </Modal>
  );
};
