'use client';

import { createTenantSchema } from '@ecommerce/core';
import { Button, Field, FieldError, FieldGroup, FieldLabel, Input, Modal } from '@ecommerce/ui';
import { zodResolver } from '@hookform/resolvers/zod';
import { Controller, useForm } from 'react-hook-form';

import { useModalState } from '@shared/state/use-modal-state';

import type * as z from 'zod';

export const TenantModal = () => {
  const { isOpen, closeModal } = useModalState('tenant');

  type TenantFormValues = z.infer<typeof createTenantSchema>;
  const form = useForm<TenantFormValues>({
    resolver: zodResolver(createTenantSchema),
    defaultValues: {
      tenant: '',
      store: '',
    },
  });

  const onSubmit = (values: TenantFormValues) => {
    console.warn(values);
  };

  return (
    <Modal
      title="Onboarding Tenant"
      description="Add new tenant and store to manage your products"
      isOpen={isOpen}
      onClose={closeModal}
      footer={
        <>
          <Button type="button" variant="outline" onClick={closeModal}>
            Cancel
          </Button>
          <Button type="submit" form="tenant-modal-form">
            Continue
          </Button>
        </>
      }
    >
      <form id="tenant-modal-form" onSubmit={form.handleSubmit(onSubmit)}>
        <FieldGroup>
          <Controller
            name="tenant"
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel htmlFor={field.name}>Tenant</FieldLabel>
                <Input
                  {...field}
                  id={field.name}
                  type="text"
                  placeholder="My Tenant"
                  autoComplete="off"
                  aria-invalid={fieldState.invalid}
                />
                {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
              </Field>
            )}
          />
          <Controller
            name="store"
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel htmlFor={field.name}>Store</FieldLabel>
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
