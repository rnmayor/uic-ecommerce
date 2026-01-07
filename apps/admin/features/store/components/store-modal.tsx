'use client';

import { createStoreSchema } from '@ecommerce/core';
import { Button, Field, FieldError, FieldGroup, FieldLabel, Input, Modal } from '@ecommerce/ui';
import { zodResolver } from '@hookform/resolvers/zod';
import { Controller, useForm } from 'react-hook-form';

import { useModalState } from '@shared/state/use-modal-state';

import type * as z from 'zod';

export const StoreModal = () => {
  const { isOpen, closeModal } = useModalState('store');

  type StoreFormValues = z.infer<typeof createStoreSchema>;
  const form = useForm<StoreFormValues>({
    resolver: zodResolver(createStoreSchema),
    defaultValues: {
      name: '',
    },
  });

  const onSubmit = (values: StoreFormValues) => {
    console.warn(values);
  };

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
          <Button type="submit" form="store-modal-form">
            Continue
          </Button>
        </>
      }
    >
      <form id="store-modal-form" onSubmit={form.handleSubmit(onSubmit)}>
        <FieldGroup>
          <Controller
            name="name"
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel htmlFor={field.name}>Name</FieldLabel>
                <Input
                  {...field}
                  id={field.name}
                  type="text"
                  placeholder="My Store"
                  autoComplete="off"
                  aria-invalid={fieldState.invalid}
                />
                {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
              </Field>
            )}
          />
        </FieldGroup>
      </form>
    </Modal>
  );
};
