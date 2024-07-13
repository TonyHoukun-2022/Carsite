'use client'

import { Button } from 'flowbite-react'
import React from 'react'
import { signIn } from 'next-auth/react';

export default function LoginBtn() {
  return (
    <Button outline onClick={() => signIn('id-server', { callbackUrl: '/' })}>
      Login
    </Button>
  )
}