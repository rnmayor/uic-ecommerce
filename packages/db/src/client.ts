import 'dotenv/config';
import { PrismaPg } from '@prisma/adapter-pg';
import { withAccelerate } from '@prisma/extension-accelerate';
import { env } from 'prisma/config';

import { PrismaClient } from './generated/prisma/client';

// Create extended client type
const adapter = new PrismaPg({ connectionString: env('DATABASE_URL') });
const _prismaClient = new PrismaClient({ adapter }).$extends(withAccelerate());
type PrismaExtended = typeof _prismaClient;

declare global {
  // Avoid multiple instances during hot reloads
  var prisma: PrismaExtended | undefined;
}

// Use global singleton
export const prisma: PrismaExtended = global.prisma || _prismaClient;

if (process.env.NODE_ENV !== 'production') {
  global.prisma = prisma;
}
