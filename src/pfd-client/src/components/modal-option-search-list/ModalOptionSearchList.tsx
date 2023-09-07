import { Container, Grid } from "@mantine/core";
import { useState } from "react";
import { SearchBox } from "../search-box/SearchBox";
import { ModalButton } from "../modal-button/ModalButton";

// TODO: Handle list overflow
export const ModalOptionSearchList = ({
  options,
  searchPlaceholder,
  truncateOptionDescription,
  optionDescriptionWidth,
  onOptionSelect,
}: ModalOptionSearchListProps) => {
  const [searchFilter, setSearchFilter] = useState("");
  const filteredOptions = options.filter((option) =>
    searchFilter.length == 1
      ? option.description.toLowerCase().startsWith(searchFilter.toLowerCase())
      : option.description.toLowerCase().includes(searchFilter.toLowerCase())
  );

  const onFilterUpdate = (filter: string) => {
    setSearchFilter(filter);
  };

  // TODO: The container sizing should be more dynamic?
  return (
    <Container p={0} w={700} h={300} size="xl">
      <SearchBox
        pl="xs"
        pr="xs"
        placeholder={searchPlaceholder ? searchPlaceholder : "Filter ..."}
        actionIconActive={false}
        value={searchFilter}
        onChange={(event) => onFilterUpdate(event.currentTarget.value)}
      />
      <Grid pt="md" pb="md" pl="md" pr="md">
        {filteredOptions.map((option) => {
          return (
            <Grid.Col span={6} key={option.key}>
              <ModalButton
                icon={option.icon}
                iconHeight="32px"
                iconPosition="left"
                description={option.description}
                includeChevron={true}
                padding="md"
                truncateDescription={truncateOptionDescription}
                textWidth={optionDescriptionWidth}
                action={() => onOptionSelect && onOptionSelect(option.key)}
              />
            </Grid.Col>
          );
        })}
      </Grid>
    </Container>
  );
};

interface ModalOptionSearchListProps {
  options: Array<{ key: string; description: string; icon: JSX.Element }>;
  truncateOptionDescription?: boolean;
  optionDescriptionWidth?: number;
  searchPlaceholder?: string;
  onOptionSelect?: (optionKey: string) => void;
}
