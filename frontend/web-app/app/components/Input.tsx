import React from 'react'
import { Label, TextInput } from 'flowbite-react'
import { UseControllerProps, useController } from 'react-hook-form'

type Props = {
  label: string
  type?: string
  showLabel?: boolean
} & UseControllerProps

export default function Input(props: Props) {
  /**
   * field: {
   *  name,
   *  value,
   *  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void,
   *  onBlur: (e: React.FocusEvent<HTMLInputElement>) => void
   *  focus: () => void
   *  blur
   * }
   */

  /**
   * fieldState: {
   *  error: FieldError | undefined
   *  isTouched,
   *  isDirty,
   *  isValid,
   *  isSubmitted,
   *  isSubmitting
   * }
   */

  /**
   * interface FieldError {
      type: string; // The type of error (e.g., "required", "minLength", "maxLength", "pattern")
       message?: string; // An optional message describing the error
      ref?: React.RefObject<any>; // A reference to the field's DOM element
}
   */
  const { fieldState, field } = useController({ ...props, defaultValue: '' })

  return (
    <div className='mb-3'>
      {props.showLabel && (
        <div className='mb-2 block'>
          <Label htmlFor={field.name} value={props.label} />
        </div>
      )}
      <TextInput
        {...props}
        {...field}
        type={props.type || 'text'}
        placeholder={props.label}
        color={fieldState.error ? 'failure' : !fieldState.isDirty ? '' : 'success'}
        helperText={fieldState.error?.message}
      />
    </div>
  )
}