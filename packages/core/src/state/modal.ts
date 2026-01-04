import type { StateCreator } from 'zustand';

export type ModalState<TModal extends string> = {
  activeModal: TModal | null;
  openModal: (modal: TModal) => void;
  closeModal: () => void;
};

export const createModalState =
  <TModal extends string>(): StateCreator<ModalState<TModal>> =>
  (set) => ({
    activeModal: null,
    openModal: (modal) => set({ activeModal: modal }),
    closeModal: () => set({ activeModal: null }),
  });
