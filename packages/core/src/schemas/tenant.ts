import * as z from 'zod';

const tenantBaseSchema = z
  .object({
    tenant: z.string().min(1, {
      message: 'Tenant name is required',
    }),
    store: z.string().min(1, {
      message: 'Store name is required',
    }),
  })
  .strict();

export const createTenantSchema = tenantBaseSchema;
// Fields are optional, but if present, must not be empty
export const updateTenantSchema = tenantBaseSchema.partial();
