import * as z from 'zod';

const storeMutableSchema = z
  .object({
    name: z.string().min(1, {
      message: 'Store name is required',
    }),
  })
  .strict();

export const createStoreSchema = storeMutableSchema;
export const updateStoreSchema = storeMutableSchema.partial(); // Makes all fields optional
