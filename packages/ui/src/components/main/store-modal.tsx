'use client';

import { Button } from '../ui';
import { Modal } from './modal';

export const StoreModal = () => {
  // TODO: state form logic here
  return (
    <Modal
      title="Create Store"
      description="Add new store to manage your products"
      isOpen={true}
      onClose={() => {}}
      footer={
        <>
          <Button type="button" variant="outline">
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
