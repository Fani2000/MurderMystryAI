import React, { useState } from 'react';
import { useMutation, gql } from '@apollo/client';
import {
  Box,
  Button,
  Typography,
  TextField,
  List,
  ListItem,
  Paper,
} from '@mui/material';
import client from './apollo-client';

// GraphQL mutation for `GenerateMurderMysteryAsync`
const GENERATE_MURDER_MYSTERY = gql`
  mutation GenerateMurderMystery($users: [UserInput!]!) {
    generateMurderMystery(users: $users) {
      fullStory
      killer
      userStories {
        key,
        value
      }
    }
  }
`;

function App() {
  const [participants, setParticipants] = useState([]);
  const [name, setName] = useState('');
  const [generatedStory, setGeneratedStory] = useState(null);

  const [generateMystery, { loading, error }] = useMutation(
    GENERATE_MURDER_MYSTERY,
    { client }
  );

  const handleAddParticipant = () => {
    if (name.trim()) {
      setParticipants([...participants, { name }]);
      setName('');
    }
  };

  const handleGenerate = async () => {
    try {
      const { data } = await generateMystery({
        variables: { users: participants },
      });
      console.log('Generated story:', data.generateMurderMystery);
      setGeneratedStory(data.generateMurderMystery);
    } catch (err) {
      console.error('Error generating story:', err);
    }
  };

  return (
    <Box sx={{ padding: 4 }}>
      {/* Title */}
      <Typography variant="h4" gutterBottom>
        Murder Mystery Generator
      </Typography>

      {/* Main Container for Input and Results (Flex Row) */}
      <Box
        sx={{
          display: 'flex',            // Enables Flexbox
          flexDirection: 'row',       // Displays input and results in a row
          alignItems: 'flex-start',   // Aligns items at the top
          justifyContent: 'space-between', // Adds space between the two sections
          gap: 4,                     // Adds spacing between sections
          marginTop: 4,
        }}
      >
        {/* Left Column: Add Participants and List */}
        <Box
          sx={{
            flex: 1,                 // Takes 1 fraction of space in the row
            display: 'flex',
            flexDirection: 'column', // Stacks content vertically in the left column
            gap: 2,                  // Adds spacing between items
          }}
        >
          {/* Add Participants Section */}
          <Box>
            <Typography variant="h6" gutterBottom>
              Add Participants
            </Typography>

            <Box
              sx={{
                display: 'flex',       // Enables Flexbox
                flexDirection: 'row',  // Arranges input and button in a row
                alignItems: 'center',  // Centers items vertically
                gap: 2,                // Adds spacing between input and button
              }}
            >
              <TextField
                label="Participant Name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                onKeyDown={(e) =>
                  e.key === 'Enter' ? handleAddParticipant() : null
                }
              />

              <Button
                variant="contained"
                onClick={handleAddParticipant}
              >
                Add
              </Button>
            </Box>
          </Box>

          {/* Participant List (Flex Row) */}
          <List
            sx={{
              display: 'flex',       // Enables Flexbox
              flexDirection: 'row',  // Makes items appear side by side
              flexWrap: 'wrap',      // Wraps to a new line if items overflow
              gap: 2,                // Adds spacing between items
              marginTop: 2,
            }}
          >
            {participants.map((participant, index) => (
              <ListItem
                key={index}
                sx={{
                  display: 'flex',            // Ensures item is flex-aware
                  alignItems: 'center',
                  justifyContent: 'center',
                  border: '1px solid #ddd',   // Adds a border
                  padding: 1,
                  borderRadius: 1,
                  width: 'auto',              // Adjusts dynamically to content
                }}
              >
                {participant.name}
              </ListItem>
            ))}
          </List>

          {/* Generate Mystery Button */}
          <Button
            variant="contained"
            color="primary"
            sx={{ marginTop: 2 }}
            onClick={handleGenerate}
            disabled={participants.length < 2 || loading}
          >
            Generate Mystery
          </Button>

          {/* Error Message */}
          {error && <Typography color="error">{error.message}</Typography>}
        </Box>

        {/* Right Column: Generated Results */}
        {generatedStory && (
          <Box
            sx={{
              flex: 2,                 // Takes 2 fractions of space in the row
              display: 'flex',
              flexDirection: 'column', // Stacks content vertically
              gap: 2,                  // Adds spacing between sections
            }}
          >
            {/* Generated Story */}
            <Paper sx={{ padding: 2 }}>
              <Typography variant="h5" gutterBottom>
                Story Result
              </Typography>
              <Box sx={{ marginBottom: 2 }}>
                <Typography variant="body1">
                  <strong>Full Story:</strong> {generatedStory.fullStory}
                </Typography>
              </Box>
              <Typography variant="h6">
                Killer: {generatedStory.killer}
              </Typography>
            </Paper>

            {/* User Stories */}
            <Box>
              <Typography variant="h6" gutterBottom>
                User Stories
              </Typography>
              {generatedStory.userStories.map((story) => (
                <Paper
                  key={story.key}
                  sx={{
                    padding: 1,
                    backgroundColor: '#f1f1f1',
                    marginBottom: 1,
                  }}
                >
                  <Typography variant="body2">
                    <strong>{story.key}:</strong> {story.value}
                  </Typography>
                </Paper>
              ))}
            </Box>
          </Box>
        )}
      </Box>
    </Box>
  );
}

export default App;