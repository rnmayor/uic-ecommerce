import { SignedIn, UserButton } from '@clerk/nextjs';
import { db } from '@ecommerce/db';
import { Button, ThemeToggler } from '@ecommerce/ui';
import { redirect } from 'next/navigation';

export default async function DashboardPage({ params }: { params: Promise<{ storeId: string }> }) {
  const { storeId } = await params;
  const store = await db.store.findFirst({
    where: {
      id: storeId,
    },
  });

  if (!store) {
    // fallback if invalid storeId in URL
    redirect('/');
  }

  return (
    <>
      <h1>Dashboard Page</h1>
      <h2>Active Store: {store.name}</h2>
      <div className="flex flex-col items-center justify-center">
        <SignedIn>
          <UserButton />
        </SignedIn>
        <ThemeToggler />
        <Button variant="default">Shadcn Button from ui package - default</Button>
        <Button variant="destructive">Shadcn Button from ui package - destructive</Button>
        <Button variant="outline">Shadcn Button from ui package - outline</Button>
        <Button variant="secondary">Shadcn Button from ui package - secondary</Button>
        <Button variant="ghost">Shadcn Button from ui package - ghost</Button>
        <Button variant="link">Shadcn Button from ui package - link</Button>

        <Button size="xs">button-xs</Button>
        <Button size="sm">button-sm</Button>
        <Button size="sm">button-lg</Button>
        <Button size="icon">icon</Button>
        <Button size="icon-xs">xs</Button>
        <Button size="icon-sm">sm</Button>
        <Button size="icon-lg">lg</Button>
      </div>
    </>
  );
}
