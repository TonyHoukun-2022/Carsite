'use server'

import { Auction, PageResult } from '@/types'

export async function getData(query: string): Promise<PageResult<Auction>> {
  const res = await fetch(`http://localhost:6001/search${query}`)

  // console.log(res.url)

  if (!res.ok) throw new Error('Failed to fetch data')

  return res.json()
}
