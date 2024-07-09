// for hook useState
'use client'

import Image from 'next/image'
import React, { useState } from 'react'

type Props = {
  imageUrl: string
}

export default function CarImage({ imageUrl }: Props) {
  const [isLoading, setLoading] = useState<boolean>(true)
  return (
    <Image
      src={imageUrl}
      alt="image"
      // img fill the parent container
      fill
      className={`
        object-cover
        group-hover:opacity-75
        duration-700
        ease-in-out
        ${isLoading ? 'grayscale blur-2xl scale-110' : 'grayscale-0 blur-0 scale-100'}
      `}
      priority
      //defining how large the image should be at different viewport widths. 
      sizes='(max-width:768px) 100vw, (max-width: 1200px) 50vw, 25vw'
      onLoad={() => setLoading(false)}
    />
  )
}
