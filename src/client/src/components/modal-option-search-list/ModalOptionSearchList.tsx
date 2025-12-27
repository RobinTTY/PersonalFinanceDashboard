import { useCallback, useState } from 'react';
import { Container, SimpleGrid } from '@mantine/core';
import { SearchBox } from '@components/search-box/SearchBox';
import { ModalButton } from '@components/modal-button/ModalButton';
import { FixedSizeList } from 'react-window';
import { ModalOptionSearchListProps, Option, RowProps } from './ModalOptionSearchListProps';

import classes from './ModalOptionSearchList.module.css';

export const ModalOptionSearchList = ({
  options,
  displayOptions,
  searchPlaceholder,
  truncateOptionDescription,
  optionDescriptionWidth,
  onOptionSelect,
}: ModalOptionSearchListProps) => {
  const [searchFilter, setSearchFilter] = useState('');
  const filteredOptions = options.filter(
    (option) =>
      option.description.toLowerCase().includes(searchFilter.toLowerCase()) ||
      option.key.toLowerCase().includes(searchFilter.toLowerCase())
  );
  const onFilterUpdate = (filter: string) => {
    setSearchFilter(filter);
  };

  const Row = ({ index, style }: RowProps) => {
    const option1 = filteredOptions[index * 2];
    const option2 = filteredOptions[index * 2 + 1];

    const getModalButton = useCallback(
      (option: Option) => (
        <ModalButton
          icon={option.icon}
          iconHeight={displayOptions?.iconHeight ?? '32px'}
          iconWidth={displayOptions?.iconWidth ?? '32px'}
          iconPosition="left"
          description={option.description}
          includeChevron
          padding="md"
          truncateDescription={truncateOptionDescription}
          textWidth={optionDescriptionWidth}
          action={() => onOptionSelect && onOptionSelect(option.key)}
        />
      ),
      []
    );

    return (
      <div style={style}>
        <SimpleGrid cols={2} pl="md" pr="sm">
          {option1 && getModalButton(option1)}
          {option2 && getModalButton(option2)}
        </SimpleGrid>
      </div>
    );
  };

  return (
    <Container p={0} w={750} h={300} size="xl">
      <SearchBox
        pl="xs"
        pr="xs"
        pb="md"
        placeholder={searchPlaceholder ?? 'Filter ...'}
        actionIconActive={false}
        value={searchFilter}
        onChange={(event) => onFilterUpdate(event.currentTarget.value)}
      />
      <FixedSizeList
        className={classes.list}
        height={230}
        width={740}
        itemSize={80}
        itemCount={Math.ceil(filteredOptions.length / 2)}
      >
        {Row}
      </FixedSizeList>
    </Container>
  );
};
