import { auth } from '@clerk/nextjs/server';
import { db } from '@ecommerce/db';
import { redirect } from 'next/navigation';

import { NoStoreHandler } from '@features/store/components/no-store-handler';

export default async function OnboardingPage() {
  const { userId } = await auth();
  const store = await db.store.findFirst({
    where: {
      userId: userId!, // userId is guaranteed by layout
    },
  });
  if (store) {
    redirect(`/${store.id}`);
  }

  return <NoStoreHandler />;
}
