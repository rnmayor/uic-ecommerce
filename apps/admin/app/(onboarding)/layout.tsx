import { auth } from '@clerk/nextjs/server';
import { redirect } from 'next/navigation';

export default async function OnboardingLayout({ children }: { children: React.ReactNode }) {
  const { userId } = await auth();
  // Ensure user is authenticated
  if (!userId) {
    redirect('/sign-in');
  }
  return <>{children}</>;
}
