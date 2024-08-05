'use client'

import { Button } from 'flowbite-react'
import React from 'react'
import { signIn } from 'next-auth/react';

export default function LoginBtn() {
  return (
    // prompt option forces the authentication provider to show the login prompt, 
    <Button outline onClick={() => signIn('id-server', { callbackUrl: '/' }, { prompt: 'login' })}>
      Login
    </Button>
  )
}